import { TestBed } from '@angular/core/testing';
import {
  HttpClientTestingModule,
  HttpTestingController,
  TestRequest
} from '@angular/common/http/testing';
import { StoryService } from './story.service';
import { environment } from '../environments/environment';

describe('StoryService', () => {
  let svc: StoryService;
  let http: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule] });
    svc = TestBed.inject(StoryService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  function expectUrlWithParams(req: TestRequest, {
    page, pageSize, query
  }: { page: string; pageSize: string; query?: string }) {
    const full = req.request.urlWithParams;
    expect(full.startsWith(`${environment.apiUrl}/stories`)).toBeTrue();

    const u = new URL(full);
    expect(u.searchParams.get('page')).toBe(page);
    expect(u.searchParams.get('pageSize')).toBe(pageSize);
    if (query !== undefined) {
      expect(u.searchParams.get('query')).toBe(query);
    } else {
      expect(u.searchParams.has('query')).toBeFalse();
    }
  }

  it('calls /api/stories with paging and query', () => {
    svc.getNewest('abc', 2, 10).subscribe();

    const req = http.expectOne(r =>
      r.method === 'GET' &&
      r.urlWithParams.startsWith(`${environment.apiUrl}/stories`)
    );

    expectUrlWithParams(req, { page: '2', pageSize: '10', query: 'abc' });
    req.flush({ items: [], total: 0, page: 2, pageSize: 10 });
  });

  it('omits query when blank', () => {
    svc.getNewest('', 1, 20).subscribe();

    const req = http.expectOne(r =>
      r.method === 'GET' &&
      r.urlWithParams.startsWith(`${environment.apiUrl}/stories`)
    );

    expectUrlWithParams(req, { page: '1', pageSize: '20' });
    req.flush({ items: [], total: 0, page: 1, pageSize: 20 });
  });
});

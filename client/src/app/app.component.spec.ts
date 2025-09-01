import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { AppComponent } from './app.component';
import { of, Subject } from 'rxjs';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';

class MockStoryService {
  responses = new Subject<any>();
  getNewest(query: string, page: number, pageSize: number){
    return this.responses.asObservable();
  }
}

function paged(items: any[], total: number, page: number, pageSize: number){
  return { items, total, page, pageSize };
}

describe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  let service: MockStoryService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AppComponent],
      imports: [FormsModule],
      providers: [{ provide: (await import('./story.service')).StoryService, useClass: MockStoryService }]
    }).compileComponents();

    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    service = TestBed.inject((await import('./story.service')).StoryService) as any;
  });

  it('loads and renders the first page of stories', fakeAsync(async () => {
    const t1 = "Test Title1"
    const t2 = "Test Title2"
    fixture.detectChanges();
    service.responses.next(paged([
      { id: 1, title: t1, url: 'http://test.com', by: 'Testy McTestFace', time: new Date().toISOString() },
      { id: 2, title: t2, url: null, by: 'Joe Test', time: new Date().toISOString() }
    ], 5, 1, 20));

    tick();
    fixture.detectChanges();

    const lis = fixture.debugElement.queryAll(By.css('ul li'));
    expect(lis.length).toBe(2);
    const a0 = lis[0].query(By.css('a'));
    expect(a0.nativeElement.textContent).toContain(t1);
    const a1 = lis[1].query(By.css('a'));
    expect(a1).toBeNull();
  }));

  it('search resets to page 1 and calls service again', fakeAsync(() => {
    fixture.detectChanges();
    service.responses.next(paged([], 0, 1, 20));
    tick();
    fixture.detectChanges();

    const input = fixture.debugElement.query(By.css('input[type="search"]')).nativeElement as HTMLInputElement;
    input.value = 'golang';
    input.dispatchEvent(new Event('input'));

    const btn = fixture.debugElement.query(By.css('button')).nativeElement as HTMLButtonElement;
    btn.click();

    service.responses.next(paged([{ id: 3, title: 'Test News', url: 'http://test.com', by: 'Testianne Testsky', time: new Date().toISOString() }], 1, 1, 20));
    tick();
    fixture.detectChanges();

    const lis = fixture.debugElement.queryAll(By.css('ul li'));
    expect(lis.length).toBe(1);
  }));

  it('Prev/Next enablement respects page and total', fakeAsync(() => {
    fixture.detectChanges();
    service.responses.next(paged(new Array(20).fill(0).map((_,i)=>({ id: i+1, title: 'Another Test Title'+i, url: 'http://test.com', by: 'u', time: new Date().toISOString()})), 60, 1, 20));
    tick();
    fixture.detectChanges();

    const buttons = fixture.debugElement.queryAll(By.css('button'));
    const prevBtn = buttons[buttons.length - 2].nativeElement as HTMLButtonElement;
    const nextBtn = buttons[buttons.length - 1].nativeElement as HTMLButtonElement;
    expect(prevBtn.disabled).toBeTrue();
    expect(nextBtn.disabled).toBeFalse();

    nextBtn.click();
    service.responses.next(paged(new Array(20).fill(0).map((_,i)=>({ id: 100+i, title: 'P2-'+i, url: 'http://test.com', by: 'u', time: new Date().toISOString()})), 60, 2, 20));
    tick();
    fixture.detectChanges();

    const buttons2 = fixture.debugElement.queryAll(By.css('button'));
    const prevBtn2 = buttons2[buttons2.length - 2].nativeElement as HTMLButtonElement;
    const nextBtn2 = buttons2[buttons2.length - 1].nativeElement as HTMLButtonElement;
    expect(prevBtn2.disabled).toBeFalse();
    expect(nextBtn2.disabled).toBeFalse();
  }));
});

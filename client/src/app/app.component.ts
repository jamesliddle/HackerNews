import { Component, OnInit } from '@angular/core';
import { StoryService } from './story.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Hacker News—Latest Stories';
  query: string = '';
  page: number = 1;
  pageSize: number = 20;
  total: number = 0;
  stories: any[] = [];
  loading = false;
  error: string | null = null;

  constructor(private api: StoryService) {}

  ngOnInit() { this.loadStories(); }

  loadStories() {
    this.loading = true;
    this.error = null;
    this.api.getNewest(this.query, this.page, this.pageSize).subscribe({
      next: (res: any) => {
        this.stories = res.items || [];
        this.total = res.total || 0;
        this.loading = false;
      },
      error: _ => {
        this.error = 'Failed to load stories';
        this.loading = false;
      }
    });
  }

get totalPages(): number {
  return Math.max(1, Math.ceil(this.total / this.pageSize));
}

get from(): number {
  return this.total === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
}

get to(): number {
  return Math.min(this.page * this.pageSize, this.total);
}

  search() { this.page = 1; this.loadStories(); }
  nextPage() { if (this.page * this.pageSize < this.total) { this.page++; this.loadStories(); } }
  prevPage() { if (this.page > 1) { this.page--; this.loadStories(); } }
}

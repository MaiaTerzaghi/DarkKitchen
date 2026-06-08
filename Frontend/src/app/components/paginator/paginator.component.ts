import { Component, EventEmitter, Input, OnChanges, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-paginator',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './paginator.component.html',
  styleUrls: ['./paginator.component.css'],
})
export class PaginatorComponent implements OnChanges {
  @Input({ required: true }) currentPage!: number;
  @Input({ required: true }) pageSize!: number;
  @Input({ required: true }) totalCount!: number;

  @Output() pageChange = new EventEmitter<number>();

  totalPages = 0;
  pages: number[] = [];

  get from(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get to(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  ngOnChanges(): void {
    this.totalPages = Math.ceil(this.totalCount / this.pageSize);
    this.pages = this.buildPageNumbers();
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) {
      return;
    }
    this.pageChange.emit(page);
  }

  private buildPageNumbers(): number[] {
    const maxVisible = 5;

    if (this.totalPages <= maxVisible) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }

    let start = Math.max(1, this.currentPage - 2);
    let end = Math.min(this.totalPages, start + maxVisible - 1);

    if (end - start < maxVisible - 1) {
      start = Math.max(1, end - maxVisible + 1);
    }

    return Array.from({ length: end - start + 1 }, (_, i) => start + i);
  }
}

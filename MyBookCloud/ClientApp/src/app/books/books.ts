import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { BooksService } from '../services/books.service';
import { Book as BookModel, ReadingStatus } from '../models/book.model';
import { BookFormComponent } from './book-form/book-form.component';
import { AccountService } from '../services/account.service';
import { BookSignalRService } from '../services/book-signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-books',
  standalone: false,
  templateUrl: './books.html',
  styleUrls: ['./books.css']
})
export class Books implements OnInit, OnDestroy {
  books: BookModel[] = [];
  loading = false;
  error: string | null = null;
  ReadingStatus = ReadingStatus;
  private subscriptions = new Subscription();

  constructor(
    private booksService: BooksService,
    private dialog: MatDialog,
    private accountService: AccountService,
    private bookSignalRService: BookSignalRService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.loadBooks();
    this.subscribeToRealtimeUpdates();
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
  }

  loadBooks(): void {
    this.loading = true;
    this.error = null;
    
    this.booksService.getBooks().subscribe({
      next: (data) => {
        this.books = data;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load books. Please make sure the API is running.';
        this.loading = false;
        console.error('Error loading books:', err);
      }
    });
  }

  private subscribeToRealtimeUpdates(): void {
    const sub = this.bookSignalRService.bookUpdated$.subscribe(update => {
      if (!update?.id) {
        return;
      }

      const normalizedUpdateId = update.id.toLowerCase();
      const index = this.books.findIndex(b => (b.id ?? '').toLowerCase() === normalizedUpdateId);
      if (index === -1) {
        return;
      }

      this.books[index] = {
        ...this.books[index],
        coverThumbnailUrl: update.coverThumbnailUrl ?? this.books[index].coverThumbnailUrl,
        pageCount: update.pageCount ?? this.books[index].pageCount
      };
    });

    this.subscriptions.add(sub);
  }

  addBook(): void {
    const dialogRef = this.dialog.open(BookFormComponent, {
      width: '400px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.booksService.addBook(result).subscribe({
          next: () => this.loadBooks(),
          error: (err) => console.error('Error adding book:', err)
        });
      }
    });
  }

  editBook(book: BookModel): void {
    const dialogRef = this.dialog.open(BookFormComponent, {
      width: '400px',
      data: { ...book }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result && book.id) {
        this.booksService.updateBook(book.id, result).subscribe({
          next: () => this.loadBooks(),
          error: (err) => console.error('Error updating book:', err)
        });
      }
    });
  }

  deleteBook(book: BookModel): void {
    if (book.id && confirm(`Are you sure you want to delete "${book.title}"?`)) {
      this.booksService.deleteBook(book.id).subscribe({
        next: () => this.loadBooks(),
        error: (err) => console.error('Error deleting book:', err)
      });
    }
  }

  logout(): void {
    this.accountService.logout();
    this.router.navigate(['/login']);
  }

  getStatusLabel(status: ReadingStatus): string {
    switch (status) {
      case ReadingStatus.Reading: return 'Reading';
      case ReadingStatus.WantToRead: return 'Want to Read';
      case ReadingStatus.Finished: return 'Finished';
      default: return 'Unknown';
    }
  }
}

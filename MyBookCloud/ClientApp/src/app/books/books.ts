import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BooksService } from '../services/books.service';
import { Book as BookModel, ReadingStatus } from '../models/book.model';
import { BookFormComponent } from './book-form/book-form.component';

@Component({
  selector: 'app-books',
  standalone: false,
  templateUrl: './books.html',
  styleUrl: './books.css'
})
export class Books implements OnInit {
  books: BookModel[] = [];
  loading = false;
  error: string | null = null;
  ReadingStatus = ReadingStatus;

  constructor(
    private booksService: BooksService,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadBooks();
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

  getStatusLabel(status: ReadingStatus): string {
    switch (status) {
      case ReadingStatus.Reading: return 'Reading';
      case ReadingStatus.WantToRead: return 'Want to Read';
      case ReadingStatus.Finished: return 'Finished';
      default: return 'Unknown';
    }
  }
}

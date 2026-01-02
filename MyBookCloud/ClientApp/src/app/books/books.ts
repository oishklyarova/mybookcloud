import { Component, OnInit } from '@angular/core';
import { BooksService } from '../services/books.service';
import { Book as BookModel } from '../models/book.model';

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

  constructor(private booksService: BooksService) { }

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

  refresh(): void {
    this.loadBooks();
  }
}

import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Book, ReadingStatus } from '../../models/book.model';

@Component({
  selector: 'app-book-form',
  standalone: false,
  template: `
    <h2 mat-dialog-title>{{ data ? 'Edit Book' : 'Add New Book' }}</h2>
    <mat-dialog-content>
      <form [formGroup]="bookForm" class="form-container">
        <mat-form-field appearance="fill">
          <mat-label>Title</mat-label>
          <input matInput formControlName="title" required>
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Author</mat-label>
          <input matInput formControlName="author" required>
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>ISBN</mat-label>
          <input matInput formControlName="isbn">
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Status</mat-label>
          <mat-select formControlName="status">
            <mat-option [value]="0">Reading</mat-option>
            <mat-option [value]="1">Want to Read</mat-option>
            <mat-option [value]="2">Finished</mat-option>
          </mat-select>
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Personal Rating (1-5)</mat-label>
          <input matInput type="number" formControlName="personalRating" min="1" max="5">
        </mat-form-field>

        <mat-form-field appearance="fill">
          <mat-label>Notes</mat-label>
          <textarea matInput formControlName="note" rows="3"></textarea>
        </mat-form-field>
      </form>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button (click)="onCancel()">Cancel</button>
      <button mat-raised-button color="primary" [disabled]="bookForm.invalid" (click)="onSave()">Save</button>
    </mat-dialog-actions>
  `,
  styles: [`
    .form-container {
      display: flex;
      flex-direction: column;
      gap: 10px;
      min-width: 300px;
      padding: 10px 0;
    }
    mat-form-field {
      width: 100%;
    }
  `]
})
export class BookFormComponent implements OnInit {
  bookForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    public dialogRef: MatDialogRef<BookFormComponent>,
    @Inject(MAT_DIALOG_DATA) public data: Book | null
  ) {
    this.bookForm = this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required],
      isbn: [''],
      status: [ReadingStatus.WantToRead, Validators.required],
      personalRating: [null, [Validators.min(1), Validators.max(5)]],
      note: ['']
    });
  }

  ngOnInit(): void {
    if (this.data) {
      this.bookForm.patchValue(this.data);
    }
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave(): void {
    if (this.bookForm.valid) {
      this.dialogRef.close(this.bookForm.value);
    }
  }
}


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

        <div class="readonly-section" *ngIf="data">
          <div class="readonly-header">Google Books info</div>

          <div class="readonly-content">
            <div class="cover-preview" *ngIf="data.coverThumbnailUrl; else noCoverPreview">
              <img
                [src]="data.coverThumbnailUrl"
                [alt]="data.title + ' cover'"
              />
            </div>
            <ng-template #noCoverPreview>
              <div class="cover-preview placeholder">
                <mat-icon>menu_book</mat-icon>
              </div>
            </ng-template>

            <div class="readonly-meta">
              <div class="meta-line">
                <span class="label">Pages:</span>
                <span class="value">
                  {{ data.pageCount || 'N/A' }}
                </span>
              </div>
            </div>
          </div>
        </div>

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
      min-width: 320px;
      padding: 10px 0;
    }
    mat-form-field {
      width: 100%;
    }
    .readonly-section {
      margin-top: 8px;
      padding: 12px;
      border-radius: 8px;
      background: #fafafa;
      border: 1px dashed rgba(0,0,0,0.08);
    }
    .readonly-header {
      font-size: 0.85rem;
      font-weight: 600;
      text-transform: uppercase;
      letter-spacing: 0.04em;
      color: #757575;
      margin-bottom: 8px;
    }
    .readonly-content {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .cover-preview {
      width: 64px;
      height: 96px;
      border-radius: 4px;
      overflow: hidden;
      box-shadow: 0 4px 8px rgba(0,0,0,0.2);
      background: #f5f5f5;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    .cover-preview img {
      width: 100%;
      height: 100%;
      object-fit: cover;
    }
    .cover-preview.placeholder {
      color: #bdbdbd;
    }
    .readonly-meta {
      display: flex;
      flex-direction: column;
      gap: 4px;
      font-size: 0.9rem;
      color: #616161;
    }
    .meta-line .label {
      font-weight: 500;
      margin-right: 4px;
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


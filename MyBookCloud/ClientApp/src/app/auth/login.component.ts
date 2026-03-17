import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-login',
  standalone: false,
  template: `
    <div class="login-container">
      <mat-card>
        <mat-card-title>Sign in</mat-card-title>
        <mat-card-content>
          <form [formGroup]="form" (ngSubmit)="onSubmit()">
            <mat-form-field appearance="fill">
              <mat-label>Email</mat-label>
              <input matInput formControlName="email" type="email" required />
            </mat-form-field>

            <mat-form-field appearance="fill">
              <mat-label>Password</mat-label>
              <input matInput formControlName="password" type="password" required />
            </mat-form-field>

            <div class="actions">
              <button mat-raised-button color="primary" type="submit" [disabled]="form.invalid || loading">
                Login
              </button>
            </div>
            <div class="error" *ngIf="error">{{ error }}</div>
          </form>
        </mat-card-content>
      </mat-card>
    </div>
  `,
  styles: [`
    .login-container {
      min-height: 100vh;
      display: flex;
      justify-content: center;
      align-items: center;
      background: linear-gradient(135deg, #e3f2fd, #f3e5f5);
    }
    mat-card {
      width: 360px;
      padding: 16px;
    }
    mat-form-field {
      width: 100%;
    }
    .actions {
      margin-top: 16px;
      display: flex;
      justify-content: flex-end;
    }
    .error {
      margin-top: 8px;
      color: #c62828;
      font-size: 0.9rem;
    }
  `]
})
export class LoginComponent {
  form: FormGroup;
  loading = false;
  error: string | null = null;

  constructor(
    private fb: FormBuilder,
    private accountService: AccountService,
    private router: Router
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.loading = true;
    this.error = null;

    const { email, password } = this.form.value;
    this.accountService.login(email, password).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/books']);
      },
      error: () => {
        this.loading = false;
        this.error = 'Invalid email or password';
      }
    });
  }
}


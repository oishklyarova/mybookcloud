import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { AccountService } from './account.service';
import { Book } from '../models/book.model';

@Injectable({
  providedIn: 'root'
})
export class BookSignalRService implements OnDestroy {
  private connection: signalR.HubConnection | null = null;

  private readonly bookUpdatedSubject = new BehaviorSubject<Partial<Book> | null>(null);
  readonly bookUpdated$: Observable<Partial<Book> | null> = this.bookUpdatedSubject.asObservable();

  constructor(private accountService: AccountService) {
    this.startConnection();
  }

  private startConnection(): void {
    const token = this.accountService.getToken();
    if (!token) {
      console.warn('BookSignalRService: auth token not found, skipping SignalR connection.');
      return;
    }

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/books', {
        accessTokenFactory: () => token ?? ''
      })
      .withAutomaticReconnect()
      .build();

    this.registerHandlers();

    this.connection.onreconnecting(() => {
      console.warn('BookSignalRService: reconnecting...');
    });

    this.connection.onreconnected(() => {
      console.warn('BookSignalRService: reconnected.');
    });

    this.connection.onclose(err => {
      console.warn('BookSignalRService: connection closed.', err);
    });

    this.connection
      .start()
      .then(() => console.log('BookSignalRService: connected to /hubs/books'))
      .catch((err: any) => console.error('Error while starting SignalR connection: ', err));
  }

  private registerHandlers(): void {
    if (!this.connection) {
      return;
    }

    this.connection.on('BookEnriched', (data: { bookId: string; coverThumbnailUrl: string; pageCount: number }) => {
      console.log('BookSignalRService: BookEnriched received', data);
      this.bookUpdatedSubject.next({
        id: data.bookId,
        coverThumbnailUrl: data.coverThumbnailUrl,
        pageCount: data.pageCount
      } as Partial<Book>);
    });
  }

  ngOnDestroy(): void {
    if (this.connection) {
      this.connection.stop().catch((err: any) => console.error('Error stopping SignalR connection', err));
    }
  }
}


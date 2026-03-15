export enum ReadingStatus {
  Reading = 0,
  WantToRead = 1,
  Finished = 2
}

export interface Book {
  id?: string;
  title: string;
  author: string;
  isbn?: string;
  note?: string;
  averageRating?: number;
  status: ReadingStatus;
  personalRating?: number;

  coverThumbnailUrl?: string;

  pageCount?: number;
}

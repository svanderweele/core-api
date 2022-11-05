import { Observable } from 'rxjs';

export type Urls =
  | '/authentication/register'
  | '/authentication/login'
  | '/'
  | '/casino';

export type ErrorResponse = {
  message: string;
  code: string;
};

export type ErrorMessage = {
  key: string;
};

export interface PaginationResponse {
  getItems(): unknown[];
  lastItemKey?: string;
}

export abstract class PaginationService {
  abstract getAll(props?: GetAllRequest): Observable<PaginationResponse>;
}

export type GetAllRequest = {
  startKey?: string;
  limit?: number;
};

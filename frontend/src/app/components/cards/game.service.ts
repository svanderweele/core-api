import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable, of } from 'rxjs';
import {
  GetAllRequest,
  PaginationResponse,
  PaginationService,
} from 'src/app/globals';
import { environment } from 'src/environments/environment';
import { Game, GetGamesResponse, PlayGameResponse } from './game.models';

@Injectable({
  providedIn: 'root',
})
export class GameService implements PaginationService {
  //TODO: Custom Http Client which would allow for authenticated requests easily
  constructor(private httpClient: HttpClient) {}

  getAll(props?: GetAllRequest): Observable<PaginationResponse> {
    let queryString = '';
    if (props?.startKey) {
      queryString = `?startKey=${props.startKey}`;
    }
    if (props?.limit) {
      queryString += `${queryString.length === 0 ? '?' : '&'}limit=${
        props.limit
      }`;
    }

    return this.httpClient
      .get<GetGamesResponse>(
        `${environment.gamesApiUrl}/api/games${queryString}`
      )
      .pipe(
        map((response): PaginationResponse => {
          return {
            getItems: () => {
              return response.games;
            },
            lastItemKey: response.last_evaluated_key?.value,
          };
        })
      );
  }
  getItems(index: number): Observable<Game[]> {
    throw new Error('Method not implemented.');
  }
  get(index: number): Observable<Game> {
    throw new Error('Method not implemented.');
  }

  playGame(token: string, id: string): Observable<PlayGameResponse> {
    return this.httpClient.get<PlayGameResponse>(
      `${environment.gamesApiUrl}/api/games/play/${id}`,
      { headers: new HttpHeaders({ Authorization: `Bearer ${token}` }) }
    );
  }
}

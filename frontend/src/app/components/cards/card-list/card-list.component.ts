import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { PaginationService } from 'src/app/globals';
import { Game } from '../game.models';

@UntilDestroy()
@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.css'],
})
export class CardListComponent {
  lastItemKey?: string = undefined;
  isLoading = false;
  isLastItemReached = false;
  games: Game[] = [];
  totalItems = 0;

  /**
   *
   */
  constructor(private paginationService: PaginationService) {
    this.loadGames();
  }

  onScroll(): void {
    this.loadGames();
  }

  private loadGames(): void {
    if (this.isLastItemReached || this.isLoading) {
      return;
    }

    this.isLoading = true;

    this.paginationService
      .getAll({ limit: 3, startKey: this.lastItemKey })
      .pipe(untilDestroyed(this))
      .subscribe((val) => {
        const freshGames = val.getItems() as Game[];

        this.isLastItemReached = freshGames.length < 3;
        this.games = [...this.games, ...freshGames];
        this.lastItemKey = val.lastItemKey;
        this.isLoading = false;
      });
  }
}

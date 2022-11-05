import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { PaginationService } from 'src/app/globals';
import { Game } from '../game.models';
import { DeviceDetectorService } from 'ngx-device-detector';
import { environment } from 'src/environments/environment';

@UntilDestroy()
@Component({
  selector: 'app-card-list',
  templateUrl: './card-list.component.html',
  styleUrls: ['./card-list.component.css'],
})
export class CardListComponent {
  itemsPerPage = environment.pagination.desktop;

  lastItemKey?: string = undefined;
  isLoading = false;
  isLastItemReached = false;
  games: Game[] = [];

  /**
   *
   */
  constructor(
    private paginationService: PaginationService,
    private deviceDetectorService: DeviceDetectorService
  ) {
    if (this.deviceDetectorService.isMobile()) {
      this.itemsPerPage = environment.pagination.mobile;
    }

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
      .getAll({ limit: this.itemsPerPage, startKey: this.lastItemKey })
      .pipe(untilDestroyed(this))
      .subscribe((val) => {
        const freshGames = val.getItems() as Game[];

        this.isLastItemReached = freshGames.length < this.itemsPerPage;
        this.games = [...this.games, ...freshGames];
        this.lastItemKey = val.lastItemKey;
        this.isLoading = false;
      });
  }
}

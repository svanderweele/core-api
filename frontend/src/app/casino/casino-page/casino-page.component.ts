import { Component } from '@angular/core';
import { AppService } from 'src/app/app.service';
import { GameService } from 'src/app/components/cards/game.service';
import { PaginationService } from 'src/app/globals';

@Component({
  selector: 'app-casino-page',
  templateUrl: './casino-page.component.html',
  styleUrls: ['./casino-page.component.css'],
  providers: [{ provide: PaginationService, useClass: GameService }],
})
export class CasinoPageComponent {
  constructor(private appService: AppService) {
    this.appService.setCurrentPage('Casino');
  }
}

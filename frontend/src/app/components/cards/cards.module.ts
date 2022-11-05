import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { CardListComponent } from './card-list/card-list.component';
import { CardTileComponent } from './card-tile/card-tile.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';

@NgModule({
  declarations: [CardListComponent, CardTileComponent],
  exports: [CardListComponent, CardTileComponent],
  imports: [CommonModule, InfiniteScrollModule],
})
export class CardsModule {}

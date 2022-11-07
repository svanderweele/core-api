import { HttpErrorResponse } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { ToastrService } from 'ngx-toastr';
import { catchError, of } from 'rxjs';
import { User } from 'src/app/auth/auth.models';
import { UserService } from 'src/app/auth/user.service';
import { GameService } from '../game.service';

@UntilDestroy()
@Component({
  selector: 'app-card-tile',
  templateUrl: './card-tile.component.html',
  styleUrls: ['./card-tile.component.css'],
})
export class CardTileComponent {
  @Input() thumbnail =
    'https://dev-core-game-bucket.s3.eu-west-1.amazonaws.com/bod.webp';
  @Input() name = 'Book of Dead x 2 against many others';
  @Input() id = '777170de-97e0-445c-8a8f-3f368f889c22';

  private user: User | null = null;

  constructor(
    private gameService: GameService,
    private userService: UserService,
    private toastrService: ToastrService,
    private router: Router
  ) {
    this.userService
      .getUser()
      .pipe(untilDestroyed(this))
      .subscribe((val) => {
        this.user = val;
      });
  }

  playGame(): void {
    if (this.user === null) {
      this.router.navigate(['/']);
      this.toastrService.error('You need to be logged in to play.');
      return;
    }

    this.gameService
      .playGame(this.user.token, this.id)
      .pipe(
        untilDestroyed(this),
        catchError((err: HttpErrorResponse) => {
          if (err.error.code === 'SESSION_ALREADY_STARTED') {
            this.toastrService.error(`${this.name} is already playing.`);
          }
          return of();
        })
      )
      .subscribe((response) => {
        console.log('Play Game Resposne ', response);

        if (response.game_url) {
          this.toastrService.success(`Started playing ${this.name}.`);
        }
      });
  }
}

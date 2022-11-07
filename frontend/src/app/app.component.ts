import { Component } from '@angular/core';
import { UntilDestroy, untilDestroyed } from '@ngneat/until-destroy';
import { TranslateService } from '@ngx-translate/core';
import { Observable } from 'rxjs';
import { AppService } from './app.service';

@UntilDestroy()
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'frontend';

  currentPage$ = new Observable<string>();

  constructor(
    private appService: AppService,
    private translateService: TranslateService
  ) {
    this.currentPage$ = this.appService
      .getCurrentPage()
      .pipe(untilDestroyed(this));

    this.translateService.addLangs(['en']);
    this.translateService.setDefaultLang('en');
    this.translateService.use('en');
  }
}

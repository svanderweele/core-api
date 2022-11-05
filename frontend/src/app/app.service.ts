import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AppService {
  private currentPage = new BehaviorSubject('Dashboard');

  getCurrentPage(): Observable<string> {
    return this.currentPage.asObservable();
  }

  setCurrentPage(page: string): void {
    this.currentPage.next(page);
  }
}

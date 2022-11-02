import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CasinoPageComponent } from './casino-page.component';

describe('CasinoPageComponent', () => {
  let component: AuthPageComponent;
  let fixture: ComponentFixture<AuthPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [AuthPageComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AuthPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

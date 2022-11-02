import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './navbar/navbar.component';
import { AuthModule } from './auth/auth.module';
import { HttpClientModule } from '@angular/common/http';
import { AuthGuard } from './guards/AuthGuard';
import { NotLoggedInGuard } from './guards/NotLoggedInGuard';

@NgModule({
  declarations: [AppComponent, NavbarComponent],
  imports: [BrowserModule, AppRoutingModule, AuthModule, HttpClientModule],
  providers: [AuthGuard, NotLoggedInGuard],
  bootstrap: [AppComponent],
})
export class AppModule {}

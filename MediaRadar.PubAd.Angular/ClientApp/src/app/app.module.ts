import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';
import { HomeComponent } from './home/home.component';
import { AdsListComponent } from './ads-list/ads-list.component';
import { DashBoardComponent } from './dashboard/dashboard.component';
import { UIService } from './shared/helpers/uiservice';
import { DataTablesModule } from 'angular-datatables';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AdsListComponent,
    DashBoardComponent,
    
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,    
    DataTablesModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full' },
      { path: 'list', component: AdsListComponent },
      { path: 'dashboard', component: DashBoardComponent },
    ])
  ],
  providers: [UIService],
  bootstrap: [AppComponent]
})
export class AppModule { }

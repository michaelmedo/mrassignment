import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html'
})
export class DashBoardComponent {
  public dashboard: any = null;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get(baseUrl + 'api/activity/Dashboard').subscribe(result => {
      this.dashboard = result;
    },
      error => console.error(error));
  }
}

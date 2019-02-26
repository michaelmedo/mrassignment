import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-ads-list',
  templateUrl: './ads-list.component.html'
})

export class AdsListComponent {
  public ads: Ads[];
  dtOptions: DataTables.Settings = {};
  dtTrigger: Subject<any> = new Subject<any>();

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.dtOptions = {
      pagingType: 'full_numbers',
      dom: '<"datatable-header"fl><"datatable-wrap"t><"datatable-footer"ip>',
      lengthMenu: [[50, 100, 500, -1], ["50", "100", "500", "All"]],
      pageLength: 50,
      order: [3, 'asc']
    };

    http.get<Ads[]>(baseUrl + 'api/activity/list').subscribe(result => {
      this.ads = result;
      this.dtTrigger.next();
    },
      error => console.error(error));
  }

}

interface Ads {
  Month: string;
  PublicationId: number;
  PublicationName: string;
  ParentCompany: string;
  ParentCompanyId: number;
  BrandName: string;
  BrandId: number;
  ProductCategory: string;
  AdPages: number;
  EstPrintSpend: number;
}

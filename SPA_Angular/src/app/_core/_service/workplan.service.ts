import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class WorkplanService {
  baseUrl = environment.apiUrlEC;
  scheduleSource = new BehaviorSubject<number>(0);
  currentschedule = this.scheduleSource.asObservable();
  constructor(private http: HttpClient) { }

  getGluesByScheduleId(id) {
    return this.http.get(this.baseUrl + `Workplan/getGluesByScheduleId/${id}`, {});
  }

  getGluesByScheduleIdWithQty(id, qty) {
    return this.http.get(this.baseUrl + `Workplan/GetGluesByScheduleIdWithQty/${id}/${qty}`, {});
  }

  getGluesByScheduleIdWithQtyWithLocale(id, qty, lang) {
    return this.http.get(this.baseUrl + `Workplan/GetGluesByScheduleIdWithQtyWithLocale/${id}/${qty}/${lang}`, {});
  }

  getGluesByScheduleIdPrintTreetment(id) {
    return this.http.get(this.baseUrl + `Workplan/GetGluesByScheduleIdPrintTreetment/${id}`, {});
  }

}

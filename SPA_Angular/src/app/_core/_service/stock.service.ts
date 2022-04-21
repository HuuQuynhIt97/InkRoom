import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StockService {

  baseUrl = environment.apiUrlEC;
  chemicalSource = new BehaviorSubject<number>(0);
  currentChemical = this.chemicalSource.asObservable();
  constructor(
    private http: HttpClient
  ) { }


  getInkChemicals() {
    return this.http.get(this.baseUrl + 'Chemical/GetAllInkChemical');
  }
  delete(id) {
    return this.http.delete(this.baseUrl + 'Stock/Delete/' + id);
  }
  getStocks() {
    return this.http.get(this.baseUrl + 'Stock/GetAllStock');
  }

  ScanInput(qrCode,subName, building, userid) {
    return this.http.get(`${this.baseUrl}Stock/ScanInput/${qrCode}/${subName}/${building}/${userid}`, {});
  }

  ScanOutput(qrCode,subName, building, userid) {
    return this.http.get(`${this.baseUrl}Stock/ScanQRCodeOutput/${qrCode}/${subName}/${building}/${userid}`, {});
  }

}

import { ITreatmentWay, ITreatmentWay2 } from './../_model/TreatmentWay';
import { Injectable } from '@angular/core';
import { HttpHeaders, HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ISupplier } from '../_model/Supplier';
// import { treetmentWay } from '../_model/_entities/treetmentWay';
// import { mapper } from '../_mapper/mapper';
// import { treetmentWayDto } from '../_model/_dtos/treetmentWayDto';
// import { mapper } from '../_mapper/mapper';
// import { treetmentWay } from '../_model/_entities/treetmentWay';
// import { treetmentWayDto } from '../_model/_dtos/treetmentWayDto';
const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    // 'Authorization': 'Bearer ' + localStorage.getItem('token'),
  }),
};
@Injectable({
  providedIn: 'root'
})
export class TreatmentService {
  baseUrl = environment.apiUrlEC;
  constructor(
    private http: HttpClient
  ) { }

  getAll() {
    return this.http.get<ITreatmentWay2[]>(this.baseUrl + 'Process/GetAllTreatmentWay', {});
  }

  // getAll2(){
  //   return this.http.get<treetmentWay>(`${this.baseUrl}Process/GetAllTreatmentWay`, {}).pipe(
  //     map((data) => {
  //       const result = mapper.map(data, treetmentWay, treetmentWayDto);
  //       return result;
  //     })
  //   );
  // }

  getAllProcess() {
    return this.http.get(this.baseUrl + 'Process/GetAll', {});
  }

  create(treatmentWay: ITreatmentWay) {
    return this.http.post(this.baseUrl + 'Process/CreateTreatmentWay', treatmentWay);
  }

  createProcess(process) {
    return this.http.post(this.baseUrl + 'Process/Create', process);
  }

  update(treatmentWay: ITreatmentWay) {
    return this.http.put(this.baseUrl + 'Process/UpdateTreatmentWay', treatmentWay);
  }

  updateProcess(process) {
    return this.http.put(this.baseUrl + 'Process/Update', process);
  }

  delete(id: number) {
    return this.http.delete(this.baseUrl + 'Process/DeleteTreatmentWay/' + id);
  }

  deleteProcess(id: number) {
    return this.http.delete(this.baseUrl + 'Process/Delete/' + id);
  }
}

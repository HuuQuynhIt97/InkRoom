import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ScheduleService {

  baseUrl = environment.apiUrlEC;
  scheduleSource = new BehaviorSubject<number>(0);
  currentschedule = this.scheduleSource.asObservable();
  constructor(private http: HttpClient) { }

  getAll() {
    return this.http.get(this.baseUrl + 'schedule/GetAll', {});
  }

  getAllWithDate(time) {
    return this.http.get(this.baseUrl + `schedule/getAllWithDate/${time}`, {});
  }

  getAllWorkPlan() {
    return this.http.get(this.baseUrl + 'workplan/GetAllWorkPlan', {});
  }

  getAllWorkPlanWithDate(time) {
    return this.http.get(this.baseUrl + `workplan/GetAllWorkPlanWithDate/${time}`, {});
  }

  export(model) {
    return this.http.post(this.baseUrl + `workplan/WorkPlanFailedAddExport`, model , { responseType: 'blob' });
  }

  import(file, createdBy, time) {
    const formData = new FormData();
    formData.append('UploadedFile', file);
    formData.append('CreatedBy', createdBy);
    formData.append('Time', time);
    return this.http.post(this.baseUrl + 'schedule/Import', formData);
  }

  importWorkPlan(file ,time) {
    const formData = new FormData();
    formData.append('UploadedFile', file);
    formData.append('Time', time);
    return this.http.post(this.baseUrl + 'WorkPlan/Import', formData);
  }

  reject(scheduleID, userID) {
    return this.http.post(`${this.baseUrl}schedule/reject/${scheduleID}/${userID}`, {});
  }
  release(scheduleID, userID) {
    return this.http.post(`${this.baseUrl}schedule/release/${scheduleID}/${userID}`, {});
  }

  updatePoGlue(workPlanID) {
    return this.http.post(`${this.baseUrl}workplan/UpdatePoGlue/${workPlanID}`, {});
  }

  updatePart(workPlanID, partID) {
    return this.http.post(`${this.baseUrl}workplan/UpdatePart/${workPlanID}/${partID}`, {});
  }
  ProductionDateChange(value , scheduleID) {
    return this.http.post(`${this.baseUrl}schedule/UpdateProductionDate/${value}/${scheduleID}`, {});
  }
  changeDetail(detail) {
    this.scheduleSource.next(detail);
  }

  GetDetailSchedule(scheduleID,lang) {
    return this.http.get(this.baseUrl + `schedule/GetDetailSchedule/${scheduleID}/${lang}`, {});
  }

  getPartByScheduleID(scheduleID) {
    return this.http.get(this.baseUrl + `Part/GetPartByScheduleID/${scheduleID}`, {});
  }
  getDetailScheduleEdit(scheduleID) {
    return this.http.get(this.baseUrl + `schedule/GetDetailScheduleEdit/${scheduleID}`, {});
  }

  getPONumberScheduleID(scheduleID, treatment) {
    return this.http.get(this.baseUrl + `WorkPlan/GetPONumberByScheduleID/${scheduleID}/${treatment}`, {});
  }

  getPONumberScheduleIDAndPart(scheduleID, treatment, partId) {
    return this.http.get(this.baseUrl + `WorkPlan/GetPONumberByScheduleIDAndPart/${scheduleID}/${treatment}/${partId}`, {});
  }

  getPONumberScheduleIDAndPart2(scheduleID, treatment, partId) {
    return this.http.get(this.baseUrl + `WorkPlan/GetPONumberByScheduleIDAndPart2/${scheduleID}/${treatment}/${partId}`, {});
  }

  getPrintQRcodeByWorklan(id) {
    return this.http.get(this.baseUrl + `WorkPlan/GetPrintQRcodeByWorklan/${id}`, {});
  }

  getPrintQRcodeByScheduleID(id) {
    return this.http.get(this.baseUrl + `WorkPlan/GetPrintQRcodeBySchedule/${id}`, {});
  }

  DeleteSchedule(id) {
    return this.http.delete(this.baseUrl + `schedule/Delete/${id}`, {});
  }

  EditSchedule(entity) {
    return this.http.post(this.baseUrl + 'schedule/Editschedule', entity);
  }

  CreateSchedule(entity) {
    return this.http.post(this.baseUrl + 'schedule/CreateSchedule', entity);
  }

  EditPartSchedule(entity) {
    return this.http.post(this.baseUrl + 'schedule/EditPartSchedule', entity);
  }

  addPart(entity) {
    return this.http.post(this.baseUrl + 'part/Create', entity);
  }

  deletePart(id) {
    return this.http.delete(this.baseUrl + `part/Delete/${id}` );
  }

  done(scheduleID) {
    return this.http.post(this.baseUrl + `schedule/done/${scheduleID}`, {});
  }

  approval(scheduleID, userid) {
    return this.http.post(this.baseUrl + `schedule/approve/${scheduleID}/${userid}`, {});
  }

  GetChemicalBySupplier(id) {
    return this.http.get(this.baseUrl + `Chemical/GetChemicalBySupplier/${id}`, {});
  }

}

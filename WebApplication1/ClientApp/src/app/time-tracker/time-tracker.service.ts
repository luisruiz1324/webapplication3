import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {Shift} from '../shared/models/shift';
import {CreateShiftResult} from '../shared/models/createShiftResult'


@Injectable({
  providedIn: 'root'
})
export class TimeTrackerService {
  baseUrl = 'https://localhost:7247/api/v1/';

  result: any

  constructor(private http: HttpClient) { }

  getWorkShifts(){
    console.log(this.baseUrl + 'shifts')
    return this.http.get<Shift[]>(this.baseUrl + 'shifts');
  }

  getAllWorkShifts(userId: string, isActive?: boolean){
    let params = new HttpParams();
    params = params.append('userId', userId);
    if(isActive !== undefined){
      params = params.append('isActive', isActive);
    }
    console.log('params: ' + params)
    return this.http.get<Shift[]>(this.baseUrl + 'allshifts', {params: params});
  }

  createWorkShift(isAdmin:boolean){
    console.log('final step: ' + isAdmin)
    let params = new HttpParams();
    params = params.append('isAdmin', isAdmin)

    return this.http.post<CreateShiftResult>(this.baseUrl + 'shifts', null, {params: params});
  }

  updateWorkShift(isAdmin:boolean, updateShiftType: string, shiftId?: number){
    let params = new HttpParams();
    params = params.append('isAdmin', isAdmin);
    console.log('In service: ' + updateShiftType + ' ' + shiftId);
    return this.http.put<Shift>(this.baseUrl + 'shifts', {updateShiftType, shiftId}, {params: params});
  }
}

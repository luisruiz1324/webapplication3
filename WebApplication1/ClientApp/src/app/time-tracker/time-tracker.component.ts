import { Component, OnInit } from '@angular/core';
import { TimeTrackerService } from './time-tracker.service';
import {Shift} from '../shared/models/shift';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-time-tracker',
  templateUrl: './time-tracker.component.html',
  styleUrls: ['./time-tracker.component.css']
})
export class TimeTrackerComponent implements OnInit{

  previousShifts: Shift[] = [];
  activeShifts: Shift[] = [];



  constructor(private timeTrackerService: TimeTrackerService, private toastService: ToastrService) {}

  ngOnInit(): void {
    this.getWorkShifts();
  }

  getWorkShifts(){
    this.timeTrackerService.getWorkShifts().subscribe({
      next: (response) => {
        console.log(response);
        this.previousShifts = response.filter(x => x.isActive === false);
        this.activeShifts = response.filter(x => x.isActive === true);
        console.log('previous shifts:' + JSON.stringify(this.previousShifts[0]));
        console.log(this.activeShifts.length === 0)
  },
  error: (error) => this.toastService.error(error.error)
});
  }

  createWorkShift(){
    let isAdmin = localStorage.getItem('role') === 'Admin' ? true : false;
    console.log('Is Admin? :' + isAdmin)
    this.timeTrackerService.createWorkShift(isAdmin).subscribe({
      next: (response) => {
        this.ngOnInit();
      },
      error: (error) => this.toastService.error(error.error)
    });
  }

  updateWorkShift(updateShiftType: string, shiftId?: number){
    let isAdmin = localStorage.getItem('role') === 'Admin' ? true : false;
    console.log('In component: ' + updateShiftType + ' ' + shiftId);
    this.timeTrackerService.updateWorkShift(isAdmin, updateShiftType, shiftId).subscribe({
      next: (response) => {
        this.ngOnInit();
      },
      error: (error) => this.toastService.error(error.error.error)
    });
  }
  

}

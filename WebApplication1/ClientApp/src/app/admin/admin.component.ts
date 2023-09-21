import { Component, OnInit } from '@angular/core';
import { TimeTrackerService } from '../time-tracker/time-tracker.service';
import { Shift } from '../shared/models/shift';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.css']
})
export class AdminComponent implements OnInit{

  allShifts: Shift[] = [];

  constructor(private timeTrackerService: TimeTrackerService, private toastService: ToastrService){}

  ngOnInit(): void {
    this.getAllWorkShifts('', undefined);
  }

  getAllWorkShifts(userId: string, isActive?: boolean){
    this.timeTrackerService.getAllWorkShifts(userId, isActive).subscribe({
      next: (response) => {
        console.log(response);
        this.allShifts = response;
  },
  error: (error) => {
    this.toastService.error(error.error)
    this.allShifts = []
  }
});
  }

  onSubmit(param: any){
    let isActive = this.getActiveFilter(param.isActive);
    this.getAllWorkShifts(param.userId, isActive);
     }

     getActiveFilter(isActive: string):boolean | undefined{

      if(isActive === 'false'){
        return false;
      }else if(isActive === 'true'){
        return true;
      }
      return undefined;
     }


}

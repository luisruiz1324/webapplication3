import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {

  constructor(private router: Router, private authService: AuthService, private toastService: ToastrService){
    
  }

  onSubmit(val: any){
    console.log(!val.isAdmin)
    this.authService.register(val.email, val.password, !val.isAdmin ? false : true).subscribe({
     next: (response) => {
       this.authService.setToken(response.token)
       this.router.navigateByUrl('/time-tracker')
   
     },
     error: (error) => this.toastService.error(error.error.errors[0]),
    });
     }
}

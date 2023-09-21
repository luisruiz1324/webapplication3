import { Component } from '@angular/core';
import { AuthService } from '../auth/auth.service';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';


@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  constructor(private router: Router, private authService: AuthService, private toastService: ToastrService) {}

  onSubmit(val: any){
 this.authService.login(val.email, val.password).subscribe({
  next: (response) => {
    this.authService.setToken(response.token)
    this.router.navigateByUrl('/time-tracker')

  },
  error: (error) => this.toastService.error(error.error.errors[0]),
 });
  }


}

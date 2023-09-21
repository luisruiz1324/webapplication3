import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../shared/models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private _isLoggedIn$ = new BehaviorSubject<boolean>(false)
  isLoggedIn$ = this._isLoggedIn$.asObservable();

  baseUrl = 'https://localhost:7247/api/v1/';

  constructor(private http: HttpClient) {
    const token = localStorage.getItem('token')
    this._isLoggedIn$.next(!!this.token) 
  }

  login(email:string, password:string){
    console.log(this.baseUrl + 'identity/login')

    return this.http.post<User>(this.baseUrl + 'identity/login', {email, password})
  }
  register(email:string, password:string, isAdmin:boolean){
    return this.http.post<User>(this.baseUrl + 'identity/register', {email, password, isAdmin})
  }

  setToken(token:string){
    localStorage.setItem('token', token);
    if(this.isAdmin(token)){
      localStorage.setItem('role', 'Admin')
    }else{
      localStorage.setItem('role', 'Non-Admin')
    }
    this._isLoggedIn$.next(true);
 }

 get token() {
  return localStorage.getItem('token');
 }

 signout(){
  localStorage.removeItem('role')
  localStorage.removeItem('token')
 }

 isAdmin(token: string):boolean{
    let tokenData = token.split('.')[1];
    let decodedTokenJsonData = window.atob(tokenData)
    let decodedTokenData = JSON.parse(decodedTokenJsonData)
    let Role = decodedTokenData.role
    if(Role === 'Admin'){
      return true;
    }
    return false;
  }
 
}

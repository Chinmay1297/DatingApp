import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title: string = 'Dating app';
  users: any;
  constructor(
    private accountService: AccountService
  ){}

  ngOnInit(): void {
    this.setCurrentUser();
  }

  setCurrentUser()
  {
    //const user: User = JSON.parse(localStorage.getItem('user')!);   //! at the end turns of typescript strict 
    const userString = localStorage.getItem('user');
    if(!userString) return;
    const user: User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
  
}

import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any = {};
  loggedIn = false;

  constructor(private accountService: AccountService){}

  ngOnInit(): void {
   this.getCurrentUser();
  }

  getCurrentUser()
  {
    this.accountService.currentUser$.subscribe({
      next: user => this.loggedIn = !!user,             // !!makes the expression boolean same as if user = true else false
      error: err=> console.log(err)
    })
  }

  login()
  {
    this.accountService.login(this.model).subscribe({
      next: response => {
        this.loggedIn = true;
        console.log(response);
      },
      error: err => {
        console.log(err)
      },
      complete: ()=>{
        //turn off loader
      }
    });
  }

  logout()
  {
    this.accountService.logout();
    this.loggedIn = false;
  }
}

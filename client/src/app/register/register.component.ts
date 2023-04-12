import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelRegister = new EventEmitter();
  registerForm : FormGroup = new FormGroup({});
  maxDate: Date = new Date();
  validationErrors: string[] | undefined;
  passwordStrength: string | undefined;
  passwordError: string = '';

  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private router: Router){}

  ngOnInit(): void {
    this.initializeForm();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  initializeForm()
  {
    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['', [Validators.required, this.isValidUsername()]],
      knownAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['',[Validators.required, Validators.minLength(7), this.containsNumber(), 
        this.containsLowercase(), this.containsUppercase(), this.containsSpecialCharacter()]],
      confirmPassword: ['',[Validators.required, this.matchValues('password')]],
    });

    this.registerForm.controls['password'].valueChanges.subscribe({
      next: ()=> this.registerForm.controls['confirmPassword'].updateValueAndValidity()
    })
  }

  isValidUsername()
  {
    return (control: AbstractControl) => {
      if(control.value.includes(' '))
      {
        return {invalidUsername: true}
      }

      if(/[-+_!@#$%^&*.,?]/.test(control.value))
      {
        return {invalidUsername: true}
      }
      return null;
    }
  }

  matchValues(matchTo: string) : ValidatorFn{
    return (control: AbstractControl) => {
      return control.value === control.parent?.get(matchTo)?.value ? null : {notMatching: true}
    }
  }

  containsNumber()
  {
    return (control: AbstractControl) => {
      return /\d/.test(control.value) ? null : {doesntContainNumber: true}
    }
  }

  containsLowercase()
  {
    return (control: AbstractControl) => {
      return /[a-z]/.test(control.value) ? null : {doesntContainLowercase: true}
    }
  }

  

  containsUppercase()
  {
    return (control: AbstractControl) => {
      return /[A-Z]/.test(control.value) ? null : {doesntContainUppercase: true}
    }
  }

  containsSpecialCharacter()
  {
    return (control: AbstractControl) => {
      return /[-+_!@#$%^&*.,?]/.test(control.value) ? null : {doesntContainSpecialCharacter: true}
    }
  }

  register()
  {
    const dob = this.getDateOnly(this.registerForm.controls['dateOfBirth'].value);
    const values = {...this.registerForm.value, dateOfBirth: dob};
    //console.log(values);
    this.accountService.register(values).subscribe({
      next: () =>{
        this.router.navigateByUrl('/members');
      },
      error: error => {
        this.validationErrors = error
      }
    });
  }

  cancel(){
    this.cancelRegister.emit(false);
  }

  private getDateOnly(dob: string | undefined)
  {
    if(!dob) return;

    let theDob = new Date(dob);
    return new Date(theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())).toISOString().slice(0,10);
  }
}

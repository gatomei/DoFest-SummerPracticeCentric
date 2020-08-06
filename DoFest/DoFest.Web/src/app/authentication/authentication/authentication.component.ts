import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService, CitiesService, UserTypesService} from 'src/app/shared/services';

import { LoginModel } from '../models/login.model';
import { RegisterModel } from '../models/register.model';
import { AuthenticationService } from '../services/authentication.service';
import { UserTypeModel } from '../../shared/models/user-type.model';
import { CityModel } from '../../shared/models/city.model';
import { HttpResponse, HttpErrorResponse } from '@angular/common/http';


@Component({
  selector: 'app-authentication',
  templateUrl: './authentication.component.html',
  styleUrls: ['./authentication.component.scss'],
  providers: [AuthenticationService],
})
export class AuthenticationComponent implements OnInit{
  public isSetRegistered: boolean = false;
  public formGroup: FormGroup;
  public cities: CityModel[];
  public userTypes: UserTypeModel[];

  constructor(
    private readonly router: Router,
    private readonly authenticationService: AuthenticationService,
    private readonly formBuilder: FormBuilder,
    private readonly userService: UserService,
    private readonly citiesService: CitiesService,
    private readonly userTypeService: UserTypesService
  ) {
    this.formGroup = this.formBuilder.group({
      email: ['',[Validators.required, Validators.email]],
      password: ['',[Validators.required, Validators.minLength(8)]],
      name: ['',[Validators.required]],
      username:['',[Validators.required]],
      city:['', [Validators.required]],
      userType:['', [Validators.required]],
      age:[18, [Validators.min(18), Validators.max(99), Validators.required]],
      year:[1,[Validators.required, Validators.min(1), Validators.max(6)]]
    });
    this.userService.username.next('');
  }

  ngOnInit(): void {
    this.citiesService.getCities().subscribe((data)=>{
      this.cities = data;
      this.formGroup.get('city').setValue(this.cities[0].id);
    });

    this.userTypeService.getUserTypes().subscribe((data)=>{
      this.userTypes = data;
      this.formGroup.get('userType').setValue(this.userTypes[0].id);
    });
  }

  public setRegister(): void {
    this.isSetRegistered = !this.isSetRegistered;
    if(!this.isSetRegistered)
    {
      this.formGroup.markAsUntouched();
      console.log("Value changed");

      this.formGroup.setValue({email:'', password:'',name:'', username:'', city:this.cities[0].id, userType: this.userTypes[0].id,age:18, yearStudy:1});
    }
  }

  public authenticate(): void {
    if (this.isSetRegistered) {
      const data: RegisterModel = this.formGroup.getRawValue();
      console.log(data);
      this.authenticationService.register(data).subscribe((registerData: HttpResponse<any>) => {
        if(registerData.status == 201)
        {
          this.setRegister();
          document.getElementById('successful-register').innerHTML = "Successful register user, please log in!";
        }
      }, this.handleError);
    }
    else {
      const data: LoginModel = this.formGroup.getRawValue();
      this.authenticationService.login(data).subscribe((data: HttpResponse<any>) => {
        if(data.status == 200)
        {
          console.log(data.body);
          localStorage.setItem('userToken', JSON.stringify(data.body.token));
          console.log(localStorage.getItem('userToken'));
          this.userService.username.next(data.body.email);
          this.router.navigate(['dashboard']);
        }
        }, this.handleError);
      }
    }

  public isInvalid(form:AbstractControl)
  {
    return form.invalid && form.touched && form.dirty && this.isSetRegistered;
  }

  public handleError(responseError:HttpErrorResponse)
  {
    // de facut mai frumoasa functia
    if(responseError.status == 400)
    {
      let errorList;
      if(responseError instanceof HttpErrorResponse)
      {
        errorList = responseError.error.errors;
        for(var error in errorList)
        {
          var newError = document.createElement('div');
          newError.className = "error-item";
          newError.innerHTML = errorList[error][0];
          document.getElementById("error-list").appendChild(newError);
        }
      }
    }
  }
}

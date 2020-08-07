import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { UserModel } from '../models/user/user';
import { Observable } from 'rxjs';
import { CityModel } from 'src/app/shared/models/city.model';
import { CitiesService, RouteService } from 'src/app/shared/services';
import {MatTableDataSource} from '@angular/material/table';
import { ActivityModel } from 'src/app/activity/models/activity.model';
import { ActivityService } from 'src/app/activity/services/activity.service';
import {ActivityTypeModel} from "../models/activityType/activityType";
import { NewActivityTypeModel } from '../models/activityType/newActivityType';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  // ********** User table data **********
  userDisplayedColumns: string[] = ['Id', 'Username', 'Email', 'UserType'];
  userData: UserModel[] = null;
  userDataSource: MatTableDataSource<UserModel> = null;
  userTypeInput: string = null;

  // ********** City table data **********
  cityDisplayedColumns: string[] = ['Id', 'Name'];
  cityData: CityModel[] = null;
  cityDataSource: MatTableDataSource<CityModel> = null;

  // ********** Activity table data **********
  activityDisplayedColumns: string[] = ['Id', 'Title', "Description"];
  activityData: ActivityModel[] = null;
  activityDataSource: MatTableDataSource<ActivityModel> = null;

  // ********** ActivityType table data **********
  activityTypeDisplayedColumns: string[] = ['Id', 'Name'];
  activityTypeData: ActivityTypeModel[] = null;
  activityTypeDataSource: MatTableDataSource<ActivityTypeModel> = null;

  private backendEndpoint: string = "http://192.168.0.103:5002/api/v1/"
  private endpoints = {
    "activities": this.backendEndpoint + "activities",
    "activityType": this.backendEndpoint + "activities/types",
    "cities": this.backendEndpoint + "cities",
    "users": this.backendEndpoint + "admin"
  };

  private httpClient: HttpClient = null;
  private cityService: CitiesService = null;
  private activityService: ActivityService = null;
  private routeService: RouteService = null;

  constructor(
    httpClient: HttpClient,
    cityService: CitiesService,
    activityService: ActivityService,
    routeService: RouteService
    )
    {
    this.httpClient = httpClient;
    this.cityService = cityService;
    this.activityService = activityService;
    this.routeService = routeService;
  }

  // ********** Get data lits **********

  public getUsers(): Observable<unknown>{
    return this.httpClient.get<UserModel[]>(this.routeService.getRoute("admin", "get users"));
  }

  public getCities(): Observable<unknown>{
    return this.cityService.getCities();
  }

  public getActivities(): Observable<unknown>{
    return this.activityService.getAll();
  }

  public getActivityTypes(): Observable<ActivityTypeModel[]>{
    return this.httpClient.get<ActivityTypeModel[]>(this.routeService.getRoute("activityType", "get all"));
  }

  // ********** User admin functions **********

  public updateUserType(userId: string): Observable<unknown>{
    return this.httpClient.patch(this.routeService.getRoute("admin", "change userType", userId), null);
  }

  // ********** City admin functions **********

  public deleteCity(cityId: string): Observable<unknown>{
    return this.httpClient.delete(this.routeService.getRoute("city", "delete", cityId));
  }

  public addCity(cityModel: CityModel): Observable<unknown>{
    return this.httpClient.post<CityModel>(this.routeService.getRoute("city", "add one"), cityModel);
  }

  // ********** Activity admin functions **********

  public deleteActivity(activityId: string): Observable<unknown>{
    return this.httpClient.delete(this.routeService.getRoute("activity", "delete", activityId));
  }

  public addActivity(activityModel: ActivityModel): Observable<unknown>{
    return this.httpClient.post<ActivityModel>(this.routeService.getRoute("activity", "add one"), activityModel);
  }

  // ********** ActivityType admin functions **********

  public deleteActivityType(activityTypeId: string): Observable<unknown>{
    return this.httpClient.delete(this.routeService.getRoute("activityType", "delete", activityTypeId));
  }

  public addActivityType(activityTypeModel: NewActivityTypeModel): Observable<unknown>{
    return this.httpClient.post<ActivityTypeModel>(this.routeService.getRoute("activityType", "add one"), activityTypeModel);
  }
}

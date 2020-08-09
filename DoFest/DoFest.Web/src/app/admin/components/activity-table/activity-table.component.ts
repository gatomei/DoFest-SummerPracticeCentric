import { Component, OnInit, ViewChild } from '@angular/core';
import {MatPaginator} from '@angular/material/paginator';
import {AdminService } from '../../services/admin.service';
import { ActivityModel } from 'src/app/activity/models';
import { MatTableDataSource } from '@angular/material/table';

@Component({
  selector: 'app-activity-table',
  templateUrl: './activity-table.component.html',
  styleUrls: ['./activity-table.component.scss']
})
export class ActivityTableComponent implements OnInit {

  @ViewChild(MatPaginator, {static: true}) paginator: MatPaginator;

  constructor(
    public readonly adminService: AdminService
  ) { }

  ngOnInit(): void {
    this.adminService.getActivities().subscribe( (data: ActivityModel[]) => {
      this.adminService.activityData = data;
      this.adminService.activityDataSource = new MatTableDataSource<ActivityModel>(this.adminService.activityData);
      this.adminService.activityDataSource.paginator = this.paginator;
    });
  }

  public applyFilterTitle(event: Event){
    const filterValue = (event.target as HTMLInputElement).value;
    this.adminService.activityDataSource.filterPredicate = (data, filter): boolean =>{
      console.log(data);
      return data.name.toLowerCase().includes(filter.toLowerCase());
    };
    this.adminService.activityDataSource.filter = filterValue.trim().toLowerCase();
  }

}
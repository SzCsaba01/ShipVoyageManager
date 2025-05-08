import { Component, OnInit } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { FormGroup, FormControl } from '@angular/forms';
import { MatTableDataSource } from '@angular/material/table';
import { Store } from '@ngrx/store';
import { userRoles } from '../../constants/user-roles';
import { IShip } from '../../models/ship/ship.model';
import { ShipService } from '../../services/ship.service';
import { selectUserRole } from '../../state/authentication/auth.selector';
import { takeUntil } from 'rxjs';
import { CommonModule } from '@angular/common';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { formModulesUtil } from '../../shared-modules/form-module.util';
import { ConfirmationModalComponent } from "../../shared-components/confirmation-modal/confirmation-modal.component";
import { AddShipModalComponent } from "../../shared-components/add-ship-modal/add-ship-modal.component";
import { EditShipModalComponent } from "../../shared-components/edit-ship-modal/edit-ship-modal.component";

@Component({
  selector: 'app-ships',
  standalone: true,
  imports: [CommonModule, angularMaterialModulesUtil(), formModulesUtil(), ConfirmationModalComponent, AddShipModalComponent, EditShipModalComponent],
  templateUrl: './ships.component.html',
  styleUrl: './ships.component.scss'
})
export class ShipsComponent extends SelfUnsubscriberBase implements OnInit {
  displayedColumns: string[] = [];
  dataSource = new MatTableDataSource<any>([]);

  pageSize = 5;
  page = 0;
  totalShips = 0;
  pageSizeOptions = [5, 10, 25, 50];
  shipToDelete: IShip = {} as IShip;

  isDeleteShipModalOpen = false;
  isAddShipModalOpen = false;
  isEditShipModalOpen = false;

  isAdmin = false;

  selectedShip: IShip = {} as IShip;

  filterFormGroup = {} as FormGroup;
  searchTerm = {} as FormControl;

  constructor(
    private shipService: ShipService,
    private store: Store
  ) {
    super();
  }

  ngOnInit(): void {
    this.store.select(selectUserRole).subscribe(role => {
      this.isAdmin = role === userRoles.admin;
      this.setDisplayedColumns();
    });
    this.initializeForm();
    this.loadData();
  }

  private setDisplayedColumns(): void {
    this.displayedColumns = ['name', 'maxSpeed'];
    if (this.isAdmin) {
      this.displayedColumns.push('delete');
    }
  }

  private initializeForm() {
    this.searchTerm = new FormControl('');

    this.filterFormGroup = new FormGroup({
      searchTerm: this.searchTerm,
    });
  }

  private loadData() {
    this.shipService.getFilteredShipsPaginated(this.page, this.pageSize, this.searchTerm.value)
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((response) => {
        this.dataSource.data = response.ships;
        this.totalShips = response.totalCount;
      });
  }

  onPageChange(event: any): void{
    this.page = event.pageIndex;
    this.pageSize = event.pageSize;
    this.loadData();
  }

  applyFilter(): void {
    this.page = 0; 
    this.pageSize = 5;
    this.loadData();
  }

  onDeleteClicked(ship: IShip): void {
    this.shipToDelete = ship;
    this.isDeleteShipModalOpen = true;
  }

  onCloseDeleteModal(): void {
    this.isDeleteShipModalOpen = false;
  }

  onDeleteConfirmed(): void {
    this.shipService.deleteShip(this.shipToDelete.id).subscribe(() => {
      this.isDeleteShipModalOpen = false;
      this.loadData();
    });
  }

  onAddShipClicked(): void {
    this.isAddShipModalOpen = true;
  }

  onAddShip(): void {
    this.isAddShipModalOpen = false;
    this.loadData();
  }

  onCloseAddShipModal(): void {
    this.isAddShipModalOpen = false;
  }

  onRowClicked(ship: IShip): void {
    this.selectedShip = ship;
    this.isEditShipModalOpen = true;
  }

  onCloseEditShipModal(): void {
    this.isEditShipModalOpen = false;
  }

  onEditShip(): void {
    this.isEditShipModalOpen = false;
    this.loadData();
  }
}

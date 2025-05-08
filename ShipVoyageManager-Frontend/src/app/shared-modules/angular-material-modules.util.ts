import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatButtonModule } from '@angular/material/button';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';

export function angularMaterialModulesUtil(){
    return [
        MatToolbarModule,
        MatFormFieldModule, 
        MatInputModule, 
        MatSelectModule,
        MatPaginatorModule,
        MatTableModule,
        MatIconModule,
        MatCardModule,
        MatDatepickerModule,
        MatButtonModule,
        MatListModule
    ]
}
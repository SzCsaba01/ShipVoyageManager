import { Component } from '@angular/core';
import { angularMaterialModulesUtil } from '../../shared-modules/angular-material-modules.util';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [angularMaterialModulesUtil(), CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent {

}

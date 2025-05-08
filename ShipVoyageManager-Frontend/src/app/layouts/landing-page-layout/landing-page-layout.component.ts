import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-landing-page-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './landing-page-layout.component.html',
  styleUrl: './landing-page-layout.component.scss'
})
export class LandingPageLayoutComponent {

  constructor(
  ) {
  }

}

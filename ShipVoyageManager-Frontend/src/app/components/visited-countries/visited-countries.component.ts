import { Component, OnInit, ViewChild } from '@angular/core';
import { SelfUnsubscriberBase } from '../../utils/SelfUnsubscribeBase';
import { VisitedCountriesService } from '../../services/visited-countries.service';
import { LoadingService } from '../../services/loading.service';
import { takeUntil } from 'rxjs';
import { IVisitedCountry } from '../../models/visited-country/visitedCountry.model';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData, ChartType } from 'chart.js';


@Component({
  selector: 'app-visited-countries',
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './visited-countries.component.html',
  styleUrl: './visited-countries.component.scss'
})
export class VisitedCountriesComponent extends SelfUnsubscriberBase implements OnInit {
  @ViewChild(BaseChartDirective) chart?: BaseChartDirective;

  visitedCountries: IVisitedCountry[] = [];

  public lineChartData: ChartData<'line'> = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Visits per Month',
        borderColor: 'blue',
        backgroundColor: 'lightblue',
        tension: 0.3,
        fill: true,
      }
    ]
  };

  public lineChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    scales: {
      x: {},
      y: {
        beginAtZero: true
      }
    }
  };

  public lineChartType: 'line' = 'line';


  constructor(
    private  loadingService: LoadingService,
    private visitedCountriesService: VisitedCountriesService,
  ) {
    super();
  }

  ngOnInit(): void {
    this.loadData();
  }

  private loadData(): void {
    this.loadingService.show();
    this.visitedCountriesService.getVisitedCountries()
      .pipe(takeUntil(this.ngUnsubscribe))
      .subscribe((result) => {
        this.visitedCountries = result;
        this.prepareChartData();
        this.loadingService.hide();
      })
  }

  private prepareChartData(): void {
    const counts = new Map<string, number>();
  
    for (const item of this.visitedCountries) {
      const date = new Date(item.visitedDate); 
      const key = `${date.getFullYear()}-${(date.getMonth() + 1).toString().padStart(2, '0')}`; 
      counts.set(key, (counts.get(key) ?? 0) + 1);
    }
  
    const sortedEntries = Array.from(counts.entries()).sort(
      ([a], [b]) => new Date(a + '-01').getTime() - new Date(b + '-01').getTime()
    );
  
    this.lineChartData.labels = sortedEntries.map(([date]) => date);
    this.lineChartData.datasets[0].data = sortedEntries.map(([, count]) => count);

    this.chart?.update();
  }
  
}

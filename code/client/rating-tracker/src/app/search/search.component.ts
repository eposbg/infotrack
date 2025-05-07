import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RankingService } from './ranking.service';
import { Ranking, SearchResult } from './models';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FluidModule } from 'primeng/fluid';
import { DividerModule } from 'primeng/divider';
import { BadgeModule } from 'primeng/badge';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-search',
  imports: [
    ReactiveFormsModule,
    CommonModule,
    PanelModule,
    ButtonModule,
    InputTextModule,
    FluidModule,
    DividerModule,
    BadgeModule,
  ],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
  providers: [RankingService],
})
export class SearchComponent implements OnInit, OnDestroy {
  searchForm!: FormGroup;
  loading = false;
  searchResult?: SearchResult;
  historicalWeeklyData: Ranking[] = [];
  historicalMontlyData: Ranking[] = [];
  private destroy$ = new Subject<void>();

  get historicalWeeklyRankingEngines(): string[] {
    return [...new Set(this.historicalWeeklyData.map((x) => x.searchEngine))];
  }

  get historicalMontlyRankingEngines(): string[] {
    return [...new Set(this.historicalMontlyData.map((x) => x.searchEngine))];
  }

  constructor(
    private fb: FormBuilder,
    private rankingService: RankingService
  ) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      keyword: ['News weather   next   10 days', [Validators.required]],
      targetDomain: ['metoffice.gov.uk', [Validators.required]],
      top: ['100', [Validators.required]],
    });
  }

  onSubmit() {
    if (this.searchForm.invalid) return;

    this.loading = true;

    this.rankingService
      .search({
        keywords: this.searchForm.get('keyword')?.value,
        targetDomain: this.searchForm.get('targetDomain')?.value,
        maxResults: 100,
      })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (res) => {
          this.loading = false;
          this.searchResult = res;
          console.log(res);
        },
        error: (err) => {
          this.loading = false;
          console.error(err);
        },
      });

    this.rankingService
      .getLastWeekData(
        this.searchForm.get('keyword')?.value,
        this.searchForm.get('targetDomain')?.value
      )
      .pipe(takeUntil(this.destroy$))
      .subscribe((weeklyRates) => {
        this.historicalWeeklyData = weeklyRates;
      });

    this.rankingService
      .getLastMonthData(
        this.searchForm.get('keyword')?.value,
        this.searchForm.get('targetDomain')?.value
      )
      .pipe(takeUntil(this.destroy$))
      .subscribe((montlyRates) => {
        this.historicalMontlyData = montlyRates;
      });
  }

  onExampleKeywordsClick() {
    this.searchForm.get('keyword')?.setValue('News');
  }

  onExampleDomainClick() {
    this.searchForm.get('targetDomain')?.setValue('www.bbc.co.uk');
  }

  getHistoricalWeeklyForEngine(searchEngine: string) {
    return this.historicalWeeklyData.filter(
      (x) => x.searchEngine === searchEngine
    );
  }

  getHistoricalMontlyForEngine(searchEngine: string) {
    return this.historicalMontlyData.filter(
      (x) => x.searchEngine === searchEngine
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}

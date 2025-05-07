import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RankingService } from './ranking.service';
import { SearchResult } from './models';
import { PanelModule } from 'primeng/panel';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { FluidModule } from 'primeng/fluid';
import { DividerModule } from 'primeng/divider';
import { BadgeModule } from 'primeng/badge';

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
export class SearchComponent implements OnInit {
  searchForm!: FormGroup;
  loading = false;
  searchResult?: SearchResult;

  constructor(private fb: FormBuilder, private rankingService: RankingService) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      keyword: ['', [Validators.required]],
      targetDomain: ['', [Validators.required]],
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
  }

  onExampleKeywordsClick() {
    this.searchForm.get('keyword')?.setValue('News');
  }

  onExampleDomainClick() {
    this.searchForm.get('targetDomain')?.setValue('www.bbc.co.uk');
  }
}

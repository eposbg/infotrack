import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { SearchService } from './search.service';
import { SearchEngineResult, SearchResult } from './models';

@Component({
  selector: 'app-search',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss',
  providers: [SearchService],
})
export class SearchComponent implements OnInit {
  searchForm!: FormGroup;
  loading = false;
  searchResult?: SearchResult;

  constructor(private fb: FormBuilder, private searchService: SearchService) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      keyword: ['land registry search', [Validators.required]],
      targetDomain: ['infotrack.co.uk', [Validators.required]],
      top: ['100', [Validators.required]],
    });
  }

  onSubmit() {
    if (this.searchForm.invalid) return;

    this.loading = true;

    this.searchService
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
}

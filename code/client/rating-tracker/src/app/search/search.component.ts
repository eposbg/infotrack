import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { SearchService } from './search.service';
import { provideHttpClient } from '@angular/common/http';

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

  constructor(private fb: FormBuilder, private searchService: SearchService) {}

  ngOnInit(): void {
    this.searchForm = this.fb.group({
      keyword: ['', [Validators.required]],
      targetDomain: ['', [Validators.required]],
    });
  }

  onSubmit() {
    console.log(this.searchForm.value);
    if (this.searchForm.invalid) return;

    this.searchService
      .search({
        keywords: this.searchForm.get('keyword')?.value,
        targetDomain: this.searchForm.get('targetDomain')?.value,
        maxResults: 100,
      })
      .subscribe({
        next: (res) => {},
        error: (err) => {
          console.error(err);
        },
      });
  }
}

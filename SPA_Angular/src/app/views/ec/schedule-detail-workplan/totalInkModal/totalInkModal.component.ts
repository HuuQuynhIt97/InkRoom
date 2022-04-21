import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-totalInkModal',
  templateUrl: './totalInkModal.component.html',
  styleUrls: ['./totalInkModal.component.css']
})
export class TotalInkModalComponent implements OnInit {
  data: any
  pageSettings = { pageCount: 20, pageSizes: true, pageSize: 10 };
  @Input() result: any;
  constructor(
    public activeModal: NgbActiveModal,
  ) { }

  ngOnInit() {
    this.data = this.result
  }

}

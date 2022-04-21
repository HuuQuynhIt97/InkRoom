import { DataService } from 'src/app/_core/_service/data.service';
import {
  AfterViewInit,
  Directive,
  ElementRef,
  Input,
  OnDestroy,
  OnInit,
  Renderer2,
} from '@angular/core';
import { Subject, Subscription } from 'rxjs';


@Directive({
  selector: '[detailHeight]',
  outputs: ["detailHeight"]
})
export class DetailDirective implements AfterViewInit, OnDestroy , OnInit {
  @Input()
  detailHeight: string;
  subscription: Subscription[] = [];
  subject = new Subject<string>();
  constructor(
    private renderer: Renderer2,
    private dataService: DataService,
    private elementRef: ElementRef
    ) {
  }

  ngAfterViewInit() {
  }

  ngOnInit(): void {
    this.contentHeight(this.elementRef.nativeElement);
  }


  ngOnDestroy(){
    this.subscription.forEach(item => item.unsubscribe());
  }

  contentHeight(parent: HTMLElement) {
    var height = parent.offsetHeight;
    console.log(height);
    // data.actionId = actionId;
    // this.dataService.changeMessageTarget(data)

  }

}

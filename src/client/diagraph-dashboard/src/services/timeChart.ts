import { RefObject } from 'react';
import { NumberValue } from 'd3';
import * as d3 from 'd3';

export type DataPoint = Date | NumberValue;

export interface Margin {
    top: number;
    bottom: number;
    left: number;
    right: number;
}

export class TimeChart {
    private ref: RefObject<HTMLDivElement>;

    private readonly margin: Margin;
    private readonly height: number;
    private readonly width: number;
    private readonly padding: number;

    private readonly svg: d3.Selection<SVGSVGElement, unknown, null, undefined>;

    private x: d3.ScaleTime<number, number> | undefined = undefined;
    private y: d3.ScaleLinear<number, number> | undefined = undefined;

    // TODO: probably a dumb idea
    private drawSteps: (() => void)[] = [];

    constructor(
        ref: RefObject<HTMLDivElement>,
        height: number,
        width: number,
        margin: Margin,
        padding: number) {
        this.ref = ref;
        this.height = height;
        this.width = width;
        this.margin = margin;
        this.padding = padding;

        this.svg = d3.select(ref.current)
            .append("svg")
            .attr("width", this.width + this.margin.left + this.margin.right + this.padding)
            .attr("height", this.height + this.margin.top + this.margin.bottom + this.padding);
    }

    draw = () => {
        if (!this.x) throw 'x axis is not initialized';
        if (!this.y) throw 'y axis is not initialized';

        for (const step of this.drawSteps) {
            step();
        }
    }

    xAxis = (domain: Iterable<DataPoint>): TimeChart => {
        this.x = d3.scaleTime()
            .domain(domain)
            .range([0, this.width - 2 * this.padding - this.margin.top - this.margin.bottom]);

        return this;
    }

    yAxis = (domain: Iterable<DataPoint>): TimeChart => {
        this.y = d3.scaleLinear()
            .domain(domain)
            .range([this.height - 2 * this.padding - this.margin.top - this.margin.bottom, 0]);

        return this;
    }

    xLabel = (text: string): TimeChart => {
        this.drawSteps.push(() => {
            const moveX = this.margin.left + this.padding;
            const moveY = this.height + 2 * this.padding;

            this.svg.append("g")
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .call(d3.axisBottom(this.x!).ticks(12));

            this.svg?.append("text")
                .attr("transform", `translate(${this.width / 2}, ${this.height + this.margin.top + this.margin.bottom + this.padding})`)
                .style("text-anchor", "middle")
                .text(text);
        });

        return this;
    }

    yLabel = (ticks: number): TimeChart => {
        this.drawSteps.push(() => {
            const yFormat = d3.format("1");

            const moveX = this.margin.left + this.padding;
            const moveY = this.padding * 2 + this.margin.top + this.margin.bottom;
            this.svg.append("g")
                .attr('class', 'y axis')
                .attr('transform', `translate (${moveX}, ${moveY})`)
                .call(d3.axisLeft(this.y!).ticks(ticks).tickFormat(yFormat));
        });

        return this;
    }

    data = (data: Iterable<[number, number]>, onClickDataPoint: (data: [DataPoint, DataPoint]) => void): TimeChart =>
    {
        this.drawSteps.push(() => {
            const moveX = this.margin.left + this.padding;
            const moveY = this.padding * 2 + this.margin.bottom + this.margin.top;

            this.svg.append("path")
                .datum(data)
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .attr("fill", "none")
                .attr("stroke", "#69b3a2")
                .attr("stroke-width", 1.2)
                .attr("d", d3.line()
                    .x(d => this.x!(d[0]))
                    .y((d) => this.y!(d[1]))
                );

            const circleRSmall = 2.5;
            const circleRBig = 10;

            this.svg.append('g')
                .selectAll('dot')
                .data(data)
                .join('circle')
                .attr('transform', `translate(${moveX}, ${moveY})`)
                .attr("cx", d => this.x!(d[0]))
                .attr("cy", d => this.y!(d[1]))
                .attr('r', circleRSmall)
                .on('mouseover', function () {
                    d3.select(this).attr('r', circleRBig);
                })
                .on('mouseout', function () {
                    d3.select(this).attr('r', circleRSmall);
                })
                .on('click', (d, i) => {
                    onClickDataPoint([i[0], i[1]]);
                });
        });

        return this;
    }

    horizontalLine = (at: number, color: string): TimeChart => {
        this.drawSteps.push(() => {
            this.svg.append('line')
                .attr('x1', this.margin.left + this.padding)
                .attr('y1', this.y!(at) + this.padding * 2 + this.margin.top + this.margin.bottom)
                .attr('x2', this.width - this.margin.right - this.padding)
                .attr('y2', this.y!(at) + this.padding * 2 + this.margin.top + this.margin.bottom)
                .style('stroke-width', 1)
                .style('stroke', color)
                .style('fill', 'black');
        });

        return this;
    }

    verticalLine = (at: number, color: string, onClick: () => void): TimeChart => {
        this.drawSteps.push(() => {
            this.svg.append('line')
                .attr('x1', this.x!(at))
                .attr('y1', this.margin.top + 2 * this.padding + this.margin.top)
                .attr('x2', this.x!(at))
                .attr('y2', this.height + 2 * this.padding)
                .style('stroke-width', 2.5)
                .style('stroke', color)
                .style('fill', 'black')
                .on('mouseover', function() {
                    d3.select(this).style('stroke-width', 5.5);
                })
                .on('mouseout', function(){
                    d3.select(this).style('stroke-width', 2);
                })
                .on('click', onClick);
        });

        return this;
    }
}
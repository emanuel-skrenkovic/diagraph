import React from 'react';

export interface ErrorBoundaryProps { children?: React.ReactNode; }
interface ErrorBoundaryState { hasError: boolean }

export class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
    constructor(props: ErrorBoundaryProps) {
        super(props);
        this.state = { hasError: false };
    }

    componentDidCatch = (error: Error, errorInfo: React.ErrorInfo) => {
        this.setState({ hasError: true })
    }

    render = () => {
        if (this.state.hasError) {
            return <span>An error has occurred</span>;
        }

        return this.props.children;
    }
}
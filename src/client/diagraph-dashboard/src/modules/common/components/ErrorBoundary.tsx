import React from 'react';

export type ErrorBoundaryProps = { children?: React.ReactNode; }
type ErrorBoundaryState = { hasError: boolean }

export class ErrorBoundary extends React.Component<ErrorBoundaryProps, ErrorBoundaryState> {
    constructor(props: ErrorBoundaryProps) {
        super(props);
        this.state = { hasError: false };
    }

    componentDidCatch = (_error: Error, _errorInfo: React.ErrorInfo) => {
        this.setState({ hasError: true })
    }

    render = () => {
        if (this.state.hasError) {
            return <span>An error has occurred</span>;
        }

        return this.props.children;
    }
}
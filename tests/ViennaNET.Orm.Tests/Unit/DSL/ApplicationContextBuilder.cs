﻿namespace ViennaNET.Orm.Tests.Unit.DSL
{
  internal class ApplicationContextBuilder
  {
    private bool _addAllCommands;
    private bool _addAllEntities;
    private bool _addAllEvents;
    private string _nameQuery;
    private string _nickAllCommands;
    private string _nickAllEntiries;
    private string _nickAllEvents;
    private string _nickCommand;
    private string _nickEntity;
    private string _nickEvent;
    private string _nickNamedQuery;
    private string _nickQuery;
    private bool _withBadEntity;
    private bool _withCommand;
    private bool _withEntity;
    private bool _withEvent;
    private bool _withNamedQuery;
    private bool _withQuery;

    public ApplicationContextBuilder WithCommand(string nick = null)
    {
      _withCommand = true;
      _nickCommand = nick;
      return this;
    }

    public ApplicationContextBuilder WithNamedQuery(string name, string nick = null)
    {
      _withNamedQuery = true;
      _nickNamedQuery = nick;
      _nameQuery = name;
      return this;
    }

    public ApplicationContextBuilder WithEntity(string nick = null)
    {
      _withEntity = true;
      _nickEntity = nick;
      return this;
    }

    public ApplicationContextBuilder WithQuery(string nick = null)
    {
      _withQuery = true;
      _nickQuery = nick;
      return this;
    }

    public ApplicationContextBuilder WithBadEntity()
    {
      _withBadEntity = true;
      return this;
    }

    public ApplicationContextBuilder AddAllEntities(string nick = null)
    {
      _addAllEntities = true;
      _nickAllEntiries = nick;
      return this;
    }

    public ApplicationContextBuilder AddAllCommands(string nick = null)
    {
      _addAllCommands = true;
      _nickAllCommands = nick;
      return this;
    }

    public ApplicationContextBuilder WithIntegrationEvent(string nick = null)
    {
      _withEvent = true;
      _nickEvent = nick;
      return this;
    }

    public ApplicationContextBuilder AddAllIntegrationEvents(string nick = null)
    {
      _addAllEvents = true;
      _nickAllEvents = nick;
      return this;
    }

    public ApplicationContext Please()
    {
      return new MyApplicationContext(this);
    }

    private class MyApplicationContext : ApplicationContext
    {
      public MyApplicationContext(ApplicationContextBuilder builder)
      {
        if (builder._withCommand)
        {
          AddCommand<TestCommand>(builder._nickCommand);
        }

        if (builder._withNamedQuery)
        {
          AddNamedQuery(builder._nameQuery, builder._nickNamedQuery);
        }

        if (builder._withEntity)
        {
          AddEntity<TestEntity>(builder._nickEntity);
        }

        if (builder._withQuery)
        {
          AddCustomQuery<TestEntity>(builder._nickQuery);
        }

        if (builder._withBadEntity)
        {
          AddEntity<BadEntity>();
        }

        if (builder._addAllEntities)
        {
          AddAllEntities(builder._nickAllEntiries);
        }

        if (builder._addAllCommands)
        {
          AddAllCommands(builder._nickAllCommands);
        }

        if (builder._withEvent)
        {
          AddIntegrationEvent<TestIntegrationEvent>(builder._nickEvent);
        }

        if (builder._addAllEvents)
        {
          AddAllIntegrationEvents(builder._nickAllEvents);
        }
      }
    }
  }
}
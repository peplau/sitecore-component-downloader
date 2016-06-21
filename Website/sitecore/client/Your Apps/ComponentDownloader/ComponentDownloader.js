﻿var componentDownloaderDialog;

define(["sitecore"], function (Sitecore) {
    var ListPageDialog = Sitecore.Definitions.App.extend({
        initialized: function () {
            componentDownloaderDialog = this;

            // Expand all nodes
            //this.expandAllTreeViews();

            ////var treeView = $("." + this.TreeDsTemplate.attributes.type);

            //var rootInfo = "Alert,master,df686a84-eec4-4b13-8121-f9ab343f3a0f,/temp/IconCache/Network/16x16/home.png";
            //var rootPath = "/sitecore/layout/Sublayouts/Keystone/Components/Alert";

            //this.TreeDsTemplate.set("rootitem", "{df686a84-eec4-4b13-8121-f9ab343f3a0f}");
            //this.TreeDsTemplate.set("database", "master");

            ////treeView.attr('data-sc-rootitem', "df686a84-eec4-4b13-8121-f9ab343f3a0f");
            ////treeView.attr('data-sc-database', "master");

            ////this.TreeDsTemplate.viewModel.getRoot().removeChildren();
            ////this.TreeDsTemplate.viewModel.checkedItemIds([]);
            ////this.TreeDsTemplate.viewModel.initialized();

            //return;

            //var model = Sitecore.Definitions.Models.ControlModel.extend({
            //    initialize: function (options) {
            //        alert(456);
            //        debugger;
            //        this._super();
            //    }
            //});

            //var view = Sitecore.Definitions.Views.ControlView.extend({
            //    initialize: function (options) {
            //        alert(789);
            //        debugger;
            //        this._super();
            //    }
            //});

            //Sitecore.Speak.Factories.createComponent("ItemTreeViews", model, view, ".sc-SimpleComponent");

        },
        getAllTreeViews : function() {
            return new Array(
                this.TreeDsTemplate,
                this.TreeDsBaseTemplates,
                this.TreeDsItems,
                this.TreePlaceholderSettings
            );
        },
        visitAllTreeViews: function (callback) {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.visitTreeView(thisTree, callback);
            });
        },
        visitTreeView: function(tree, callback) {
            var domTree = $("[data-sc-id='" + tree.attributes.name + "']");
            if (domTree.length == 0)
                return;
            $.each(domTree, function (key, thisTree) {
                var rootNode = $(thisTree).dynatree("getRoot");
                rootNode.visit(callback);
            });
        },
        expandAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.expandTreeView(thisTree);
            });
            window.setTimeout(function () {
                $.each(trees, function (key, thisTree) {
                    componentDownloaderDialog.expandTreeView(thisTree);
                });
            }, 500);
            window.setTimeout(function () {
                $.each(trees, function (key, thisTree) {
                    componentDownloaderDialog.expandTreeView(thisTree);
                });
            }, 1000);
        },
        expandTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (!node.isExpanded()) {
                    if (node.isLazy()) {
                        node.toggleExpand();
                    }
                    else
                        node.expand(true);
                }
            });
        },
        collapseAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.collapseTreeView(thisTree);
            });
        },
        collapseTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.expand(false);
                }
            });
        },
        checkAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.checkTreeView(thisTree);
            });
        },
        checkTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(true);
                }
            });
        },
        uncheckAllTreeViews: function () {
            var trees = this.getAllTreeViews();
            $.each(trees, function (key, thisTree) {
                componentDownloaderDialog.uncheckTreeView(thisTree);
            });
        },
        uncheckTreeView: function (tree) {
            this.visitTreeView(tree, function (node) {
                if (node.isExpanded()) {
                    node.select(false);
                }
            });
        },
        getRenderingId: function () {
            var id = Sitecore.Helpers.url.getQueryParameters(window.location.href)['cid'];
            if (Sitecore.Helpers.id.isId(id)) {
                return id;
            }
            return null;
        },
        getTreeSourceName: function(tree) {
            return tree.viewModel.$el.attr("data-source");
        },
        getTreeSelectedNodes: function(tree) {
            var selectedNodes = new Array();
            componentDownloaderDialog.visitTreeView(tree, function (node) {
                if (node.isSelected()) {
                    selectedNodes[selectedNodes.length] = node;
                }
            });
            return selectedNodes;
        },
        getSelectedSourcesAndPaths: function () {
            var trees = this.getAllTreeViews();
            var selectedNodes = new Array();
            $.each(trees, function (key, thisTree) {
                var treeSourceName = componentDownloaderDialog.getTreeSourceName(thisTree);
                var treeSelectedNodes = componentDownloaderDialog.getTreeSelectedNodes(thisTree);
                var selectedPaths = new Array();
                for (var i = 0; i < treeSelectedNodes.length; i++)
                    selectedPaths[selectedPaths.length] = treeSelectedNodes[i].data.path;
                selectedNodes[selectedNodes.length] = {
                    sourceName: treeSourceName,
                    paths: selectedPaths
                };
            });
            return selectedNodes;
        },
        download: function (a, b, c) {
            //var selectedNodes = this.getSelectedNodes();

            //alert(selectedNodes.length);
            debugger;

            var getSelectedSourcesAndPaths = this.getSelectedSourcesAndPaths();
            var jsonToSend = JSON.stringify(getSelectedSourcesAndPaths);

            // data-sc-id="DownloadForm"
            var form = $("[data-sc-id='DownloadForm']");
            if (form.length === 0)
                return;
            form.attr("method","POST");

            $("<input/>",
            {
                id: "selectedPaths",
                name: "selectedPaths",
                type: "hidden",
                value: jsonToSend
            }).appendTo(form);
            $("<input/>",
            {
                id: "renderingId",
                name: "renderingId",
                type: "hidden",
                value: this.getRenderingId()
            }).appendTo(form);

            form.submit();
        }
    });
    return ListPageDialog;
});